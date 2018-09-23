*********************************
EvenMoreModifiers's documentation
*********************************

Introductory
============
Welcome to the official documentation of Even More Modifiers!
Please refer to the sidebar menu to navigate this documentation site.

This documentation is generously hosted by `readthedocs.io <readthedocs.io>`_
Docs are written in rst (reStructuredText) and is processed with `Sphinx python <http://www.sphinx-doc.org/en/master/>`_.
(Markdown docs supported using `MkDocs <https://www.mkdocs.org/>`_)
You can view the docs `on Github here <https://github.com/Jofairden/EvenMoreModifiers/tree/rework/docs>`_
Syntax highlighting is done by `Pygments <pygments.org>`_ which internally uses 'lexers' to generate highlighting.

Navigate using the links below or use the sidebar.

.. toctree::
   :caption: Table of contents
   :name: mastertoc
   :maxdepth: 1
   
   gettingstarted
   modifier
   modifierrarity
   modifierpool
   modifiereffect
   utilities
   advancedcontentcreation
   entitiessystem

   
Indices and tables
==================

* :ref:`genindex`
* :ref:`modindex`
* :ref:`search`

Brief overview of common classes
================================
Below is given a brief description of common classes from EMM. To learn more about these classes, visit their respective guides.

Modifier
========
____

Defines a modifier, has access to virtually any hook for items that can be used to affect the item. (this goes into effect when an item has that modifier on it)

ModifierEffect
==============
____

Effects live on the player, and they allow running code in ModPlayer hooks coming from a modifier. There is an auto delegation system behind the scenes that makes this possible.

ModifierPool
============
____

So a pool is what modifiers are drawn from when an item rolls it's modifiers. Just consider it a collection of modifiers, because that's all it is. On the item itself, all modifiers are also stored as a ModifierPool

ModifierRarity
=================
____

The combined strength of modifiers on an item form a numeric value that can be matched with a rarity, which can affect the item's name, change color etc.
If you understand these bits, you understand most core stuff of the mod besides a lot of back-end stuff making stuff possible